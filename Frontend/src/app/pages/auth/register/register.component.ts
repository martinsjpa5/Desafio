import { Component } from '@angular/core';
import { ReactiveFormsModule, Validators, FormGroup, FormBuilder, AbstractControl, ValidatorFn } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html'
})
export class RegisterComponent {

  form: FormGroup;
  loading = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      senha: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(100)]],
      confirmarSenha: ['', Validators.required]
    }, { validators: this.senhasIguais('senha', 'confirmarSenha') });
  }

  senhasIguais(senhaKey: string, confirmarKey: string): ValidatorFn {
    return (group: AbstractControl) => {
      const senha = group.get(senhaKey)?.value;
      const confirmar = group.get(confirmarKey)?.value;
      return senha === confirmar ? null : { senhasDiferentes: true };
    };
  }

  async registrar() {
    if (this.form.invalid || this.loading){
      this.form.markAllAsTouched();
      return;
    } 

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    try {
      await this.authService.register(this.form.value);
      this.successMessage = 'Registro efetuado com sucesso! Você será redirecionado...';

      setTimeout(() => {
        this.router.navigate(['/login']); 
      }, 2500);

    } catch (error: any) {
      if (error?.error?.erros) {
      this.errorMessage = error.error.erros.join('<br>');
    } else {
      this.errorMessage = error?.error?.message || 'Falha ao registrar. Verifique seus dados.';
    }
    } finally {
      this.loading = false;
    }
  }
}
