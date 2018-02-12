import { Component, OnInit } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { CookieService } from 'ng2-cookies';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { HostService } from '../../services/host.service'

@Component({
    selector: 'users',
    template: require('./users.component.html')
    //styleUrls: ['./login.component.css']
})
export class UsersComponent {

    constructor(
        private http: Http,
		public cookieService: CookieService,
		public host: HostService,
		public router: Router
    ) {}

	ngOnInit() {
		if (!this.cookieService.get('token'))
			this.router.navigate(['login']);
	}

    getProtected(): any {

        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append('Authorization', `Bearer ${this.cookieService.get("token")}`);
        let options = new RequestOptions({ headers: headers });

        this.http.get(this.host.host + "/api/account/protected", options).subscribe(data => {
            console.log(data);
        });
	}

	logOut() {
		this.cookieService.delete('token');
		this.router.navigate(['login']);
	}
}
