import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

/** Extrai a mensagem mais útil de uma resposta de erro do ASP.NET Core (Problem Details RFC 7807) */
function extractMessage(error: HttpErrorResponse): string {
  const body = error.error;

  // Problem Details: { title, detail, status, ... }
  if (body?.detail) return body.detail;
  if (body?.title && body.status) return body.title;

  // Erros de validação do ASP.NET: { errors: { Campo: ['msg'] } }
  if (body?.errors) {
    const messages = Object.values(body.errors as Record<string, string[]>)
      .flat()
      .join(' • ');
    if (messages) return messages;
  }

  // Fallback genérico por status
  if (error.status === 0)   return 'Não foi possível conectar ao servidor.';
  if (error.status === 400) return body?.message ?? 'Requisição inválida.';
  if (error.status === 401) return 'Não autorizado.';
  if (error.status === 403) return 'Acesso negado.';
  if (error.status === 404) return 'Recurso não encontrado.';
  if (error.status === 409) return body?.message ?? 'Conflito de dados.';
  if (error.status === 422) return body?.message ?? 'Dados inválidos.';
  if (error.status >= 500)  return 'Erro interno no servidor.';

  return 'Ocorreu um erro inesperado.';
}

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const message = extractMessage(error);

      snackBar.open(message, 'Fechar', {
        duration: 6000,
        horizontalPosition: 'right',
        verticalPosition: 'top',
        panelClass: ['error-snackbar'],
      });

      return throwError(() => error);
    })
  );
};
