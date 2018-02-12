import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { CookieService } from 'ng2-cookies';
import { HostService } from './host.service';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class AuthService {

    constructor(
        private http: Http,
        public cookieService: CookieService,
        public host: HostService
    ) { }

    logIn(login: string, password: string): Observable<any> {
        return this.http.post(this.host.host + '/api/account/login', {
            "email": login,
            "password": password
        });
	}

	logInVk(token: string, email: string): Observable<any> {
		return this.http.post(this.host.host + "/api/account/vk", {
			"token": token,
			"email": email
		});
	}

}