import { Routes } from '@angular/router';
import { LoginComponent } from './components/login-component/login-component';
import { AuthGuard } from './guards/auth.guard';
import { RegisterComponent } from './components/register-component/register-component';
import { HomeComponent } from './components/home-component/home-component';

export const routes: Routes = [
  { path: '', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
];
