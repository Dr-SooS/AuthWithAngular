import { Component } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { CookieService } from 'ng2-cookies';

@Component({
  selector: 'app',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

	private authWindow: Window;
	host = "http://localhost:57927/";
	login: string = "adfadsf@gmail.com";
	password: string = "_QQQwww111";

	constructor(
		private http: Http,
		public cookieService: CookieService
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
		if (message.origin !== "http://localhost:57927") return;

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
    this.http.post(this.host + 'api/account/login', {
      "email": this.login,
      "password": this.password
	}).subscribe(token => {
		this.cookieService.set('token', token.json());
		console.log(token.json());
    })
  }

  launchVkLogIn(): any {
	  this.authWindow = window.open("https://oauth.vk.com/authorize?client_id=6363088&display=page&redirect_uri=http://localhost:57927/vk-auth.html&scope=email&response_type=token"/*, null, 'width=600,height=400'*/);
	  //this.http.get("https://oauth.vk.com/authorize?client_id=6363088&redirect_uri=http://localhost:51693/api/account/vk&scope=email").subscribe(data => {
	  //  console.log(data);
	  //})
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
