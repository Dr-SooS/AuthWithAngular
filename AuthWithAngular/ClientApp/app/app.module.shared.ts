import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { CookieService } from 'ng2-cookies';
import { AuthService } from './services/auth.service';
import { HostService } from './services/host.service';


import { AppComponent } from './components/app/app.component';
import { LoginComponent } from './components/login/login.component';
import { UsersComponent } from './components/users/users.component';

@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
		UsersComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'login', pathMatch: 'full' },
            { path: 'login', component: LoginComponent },
			{ path: 'users', component: UsersComponent },
            { path: '**', redirectTo: 'login' }
        ])
    ],
    providers: [CookieService, AuthService, HostService]
})
export class AppModuleShared {
}
