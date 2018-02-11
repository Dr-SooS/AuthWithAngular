import { Component } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { CookieService } from 'ng2-cookies';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
    selector: 'protected',
    template: require('./protected.component.html')
    //styleUrls: ['./login.component.css']
})
export class ProtectedComponent {

    host = "http://localhost:61832/";

    constructor(
        private http: Http,
        public cookieService: CookieService,
    ) {}


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
