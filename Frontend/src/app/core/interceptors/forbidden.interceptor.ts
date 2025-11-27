import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const forbiddenInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError(error => {
      if (error.status === 403) {
        router.navigate(['/vitrine']);
        alert('Você não tem permissão para acessar este recurso.');
      }
      return throwError(() => error);
    })
  );
};
