import { Component } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { CookieService } from 'ng2-cookies';

@Component({
  selector: 'app',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  host = "http://localhost:57927/";
  login: string = "adfadsf@gmail.com";
  password: string = "_QQQwww111";

  constructor(
	  private http: Http,
    public cookieService: CookieService
  ) { }

  logIn(): any {
    this.http.post(this.host + 'api/account/login', {
      "email": this.login,
      "password": this.password
	}).subscribe(token => {
		this.cookieService.set('token', token.json());
		console.log(token.json());
    })
  }

  logInVk(): any {
    window.location.href = "https://oauth.vk.com/authorize?client_id=6363088&redirect_uri=http://localhost:57927/api/account/vk&scope=email";
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
