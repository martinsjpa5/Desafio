import { Component } from '@angular/core';
import { ReactiveFormsModule, Validators, FormGroup, FormBuilder } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoadingService } from '../../../core/services/loading.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {

  form: FormGroup;
  successMessage = '';
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private loadingService: LoadingService
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      senha: ['', Validators.required]
    });
  }

  async login() {
    if (this.form.invalid){
      this.form.markAllAsTouched();
      return;
    }

    this.loadingService.show();
    this.errorMessage = '';
    this.successMessage = '';

    try {
      await this.authService.login(
        this.form.value.email,
        this.form.value.senha
      );

      this.authService.saveUserEmail(this.form.value.email);

      this.successMessage = 'Login efetuado com sucesso! Você será redirecionado...';

      setTimeout(() => {
        this.router.navigate(['/vitrine']);
      }, 2500);

    } catch (error: any) {
      if (error?.error?.erros) {
        this.errorMessage = error.error.erros.join('<br>');
      }
      else {
        this.errorMessage = error?.error?.message || 'Falha ao efetuar login. Verifique seus dados.';
      }

    } finally {
      this.loadingService.hide();
    }
  }
}
