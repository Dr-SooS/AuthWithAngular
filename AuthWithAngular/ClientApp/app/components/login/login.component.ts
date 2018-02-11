import { Component } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { CookieService } from 'ng2-cookies';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
    selector: 'login',
    template: require('./login.component.html')
    //styleUrls: ['./login.component.css']
})
export class LoginComponent {

    private authWindow: Window;
    host = "http://localhost:61832/";
    login: string = "azaza@mail.ru";
    password: string = "_qqQQ11";

    constructor(
        private http: Http,
        public cookieService: CookieService,
        public authService: AuthService,
        public router: Router
    ) {
        if (window.addEventListener) {
            window.addEventListener("message", this.handleMessage.bind(this), false);
        } else {
            (<any>window).attachEvent("onmessage", this.handleMessage.bind(this));
        }
    }

    handleMessage(event: Event) {
        const message = event as MessageEvent;
        // Only trust messages from the below origin.
        if (message.origin !== "http://localhost:61832") return;

        const result = JSON.parse(message.data);
        if (!result.status) {
            console.log("error");
        }
        else {
            console.log(result.accessToken);
            this.http.post(this.host + "api/account/vk", { "token": result.accessToken, "email": result.email }).subscribe(res => {
                this.cookieService.set('token', res.json());
                console.log(res.json());
            });
        }

        this.authWindow.close();
    }

    logIn(): any {
        this.authService.logIn(this.login, this.password).subscribe(token => {
            this.cookieService.set('token', token.json());
            this.router.navigate(['protected']);
            console.log(token.json());
        })
    }

    launchVkLogIn(): any {
        this.authWindow = window.open("https://oauth.vk.com/authorize?client_id=6363088&display=page&redirect_uri=http://localhost:61832/vk-auth.html&scope=email&response_type=token");
    }


    getProtected(): any {

        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append('Authorization', `Bearer ${this.cookieService.get("token")}`);
        let options = new RequestOptions({ headers: headers });

        this.http.get(this.host + "api/account/protected", options).subscribe(data => {
            console.log(data);
        });
    }
}
