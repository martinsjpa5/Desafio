import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { TopSideBarComponent } from './core/layouts/topsider-bar.component';

export const routes: Routes = [

  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/auth/login/login.component')
        .then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/auth/register/register.component')
        .then(m => m.RegisterComponent)
  },
  {
    path: '',
    component: TopSideBarComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'vitrine',
        loadComponent: () => import('./pages/vitrine/vitrine.component').then(m => m.VitrineComponent)
      },
    ]
  },
  {
    path: 'admin',
    component: TopSideBarComponent,
    canActivate: [AuthGuard],
    data: { roles: ['Gerente'] },
    loadChildren: () => import('./pages/admin/admin.routes').then(m => m.ADMIN_ROUTES) 
  },
  {
    path: 'usuario',
    component: TopSideBarComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'compras',
        loadComponent: () => import('./pages/usuario/compras/listar-compras-usuario.component').then(m => m.ListarComprasUsuarioComponent)
      },
    ]
  },
  

  { path: '**', redirectTo: 'login' }
];
