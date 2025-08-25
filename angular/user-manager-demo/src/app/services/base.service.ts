import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from '../../enviroments/enviroment';
import { Observable } from 'rxjs';
import { FluentResult } from '../models/fluent-result.model';

export class BaseService {
  protected baseUrl = environment.apiBaseUrl;

  constructor(protected http: HttpClient, protected controller: string) {}

  protected get<T>(action: string, params?: HttpParams): Observable<FluentResult<T>> {
    return this.http.get<FluentResult<T>>(`${this.baseUrl}/${this.controller}/${action}`, { params, withCredentials: true });
  }

  protected post<T>(action: string, body: any): Observable<FluentResult<T>> {
    return this.http.post<FluentResult<T>>(`${this.baseUrl}/${this.controller}/${action}`, body, { withCredentials: true });
  }

  protected put<T>(action: string, body: any): Observable<FluentResult<T>> {
    return this.http.put<FluentResult<T>>(`${this.baseUrl}/${this.controller}/${action}`, body, { withCredentials: true });
  }

  protected delete<T>(action: string, params?: HttpParams): Observable<FluentResult<T>> {
    return this.http.delete<FluentResult<T>>(`${this.baseUrl}/${this.controller}/${action}`, { params, withCredentials: true });
  }
}
