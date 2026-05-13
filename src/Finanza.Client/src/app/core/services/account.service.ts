import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';

export interface ExportDataResponse {
  exportadoEm: string;
  perfil: { nomeCompleto: string; email: string };
  categorias: { nome: string; descricao?: string }[];
  transacoes: {
    descricao: string;
    valor: number;
    tipo: string;
    status: string;
    categoria: string;
    vencimento: string;
    pagamento?: string;
  }[];
}

@Injectable({ providedIn: 'root' })
export class AccountService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/account`;

  exportData(): Observable<ExportDataResponse> {
    return this.http.get<ExportDataResponse>(`${this.baseUrl}/export`);
  }

  deleteAccount(password: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/`, { body: { password } });
  }
}
