import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import {
  Transaction,
  CreateTransactionRequest,
  UpdateTransactionRequest,
  PayTransactionRequest,
  TransactionStatus,
  TransactionType,
} from '../models/transaction.model';

export interface TransactionFilter {
  search?: string;
  status?: TransactionStatus | '';
  type?: TransactionType | '';
  startDate?: Date | null;
  endDate?: Date | null;
}

@Injectable({ providedIn: 'root' })
export class TransactionService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/transactions`;

  getAll(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(`${this.baseUrl}/all`);
  }

  search(filter: TransactionFilter): Observable<Transaction[]> {
    let params = new HttpParams();
    if (filter.search?.trim()) params = params.set('search', filter.search.trim());
    if (filter.status) params = params.set('status', filter.status);
    if (filter.type) params = params.set('type', filter.type);
    if (filter.startDate) params = params.set('startDate', filter.startDate.toISOString());
    if (filter.endDate) params = params.set('endDate', filter.endDate.toISOString());
    return this.http.get<Transaction[]>(`${this.baseUrl}/search`, { params });
  }

  getById(id: string): Observable<Transaction> {
    return this.http.get<Transaction>(`${this.baseUrl}/${id}`);
  }

  getByStatus(status: TransactionStatus): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(`${this.baseUrl}/status/${status}`);
  }

  getByType(type: TransactionType): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(`${this.baseUrl}/type/${type}`);
  }

  create(request: CreateTransactionRequest): Observable<Transaction> {
    return this.http.post<Transaction>(`${this.baseUrl}/`, request);
  }

  update(id: string, request: UpdateTransactionRequest): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.baseUrl}/${id}`, request);
  }

  pay(id: string, request: PayTransactionRequest): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.baseUrl}/pay/${id}`, request);
  }

  reopen(id: string): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.baseUrl}/reopen/${id}`, {});
  }

  cancel(id: string): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.baseUrl}/cancel/${id}`, {});
  }

  remove(id: string): Observable<Transaction> {
    return this.http.delete<Transaction>(`${this.baseUrl}/remove/${id}`, {});
  }
}