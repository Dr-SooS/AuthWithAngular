import { Component, OnInit } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { CookieService } from 'ng2-cookies';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { HostService } from '../../services/host.service';
import { User } from '../../models/user'

@Component({
    selector: 'users',
    template: require('./users.component.html')
    //styleUrls: ['./login.component.css']
})
export class UsersComponent {

	users: User[];
	currentUser: User;

    constructor(
        private http: Http,
		public cookieService: CookieService,
		public host: HostService,
		public router: Router
    ) {}

	ngOnInit() {
		if (!this.cookieService.get('token'))
			this.router.navigate(['login']);

		let headers = new Headers();
		headers.append('Content-Type', 'application/json');
		headers.append('Authorization', `Bearer ${this.cookieService.get("token")}`);
		let options = new RequestOptions({ headers: headers });

		this.http.get(this.host.host + '/api/useractions/currentuser', options).subscribe(res => {
			this.currentUser = res.json() as User;
			console.log(this.currentUser);
		})

		this.http.get(this.host.host + '/api/users').subscribe(res => {
			this.users = res.json() as User[];
			console.log(this.users);
		})
	}

	onRoleChange(user: User) {
		this.http.post(this.host.host + '/api/useractions/role/' + user.id, user).subscribe(res => {
			user = res.json();
			console.log(user);
		})
	}

	onBlockedChange(user: User) {
		this.http.post(this.host.host + '/api/useractions/blocked/' + user.id + '?new_state=' + user.blocked, null).subscribe(res => {
			user = res.json();
			console.log(user);
		})
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
