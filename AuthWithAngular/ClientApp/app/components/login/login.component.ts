import { Component, OnInit } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { CookieService } from 'ng2-cookies';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { HostService } from '../../services/host.service'

@Component({
    selector: 'login',
    template: require('./login.component.html')
    //styleUrls: ['./login.component.css']
})
export class LoginComponent {

    private authWindow: Window;
	login: string = "adfadsf@gmail.com";
	password: string = "_QQQwww111";

	constructor(
		public http: Http,
        public cookieService: CookieService,
        public authService: AuthService,
		public router: Router,
		public host: HostService
    ) {}

	ngOnInit() {
		if (window.addEventListener) {
			window.addEventListener("message", this.handleMessage.bind(this), false);
		} else {
			(<any>window).attachEvent("onmessage", this.handleMessage.bind(this));
		}

		if (this.cookieService.get('token'))
			this.router.navigate(['users']);
	}

    handleMessage(event: Event) {
        const message = event as MessageEvent;
		// Only trust messages from the below origin.
		if (message.origin !== this.host.host) return;

        const result = JSON.parse(message.data);
        if (!result.status) {
            console.log("error");
        }
        else {
			console.log(result.accessToken);
			this.authService.logInVk(result.accessToken, result.email).subscribe(res => {
				this.cookieService.set('token', res.json());
				this.router.navigate(['users']);
                console.log(res.json());
            });
        }

        this.authWindow.close();
    }

    logIn(): any {
        this.authService.logIn(this.login, this.password).subscribe(token => {
            this.cookieService.set('token', token.json());
            this.router.navigate(['users']);
            console.log(token.json());
        })
    }

	launchVkLogIn(): any {
		this.authWindow = window.open("https://oauth.vk.com/authorize?client_id=6363088&display=page&redirect_uri="+ this.host.host + "/vk-auth.html&scope=email&response_type=token");
    }
}
