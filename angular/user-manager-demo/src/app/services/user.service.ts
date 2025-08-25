import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from './base.service';
import { FluentResult } from '../models/fluent-result.model';
import { PagedResult } from '../models/paged-result.model';
import { ReadUser } from '../models/read-user.model';
import { User } from '../models/user.model';


@Injectable({
  providedIn: 'root'
})
export class UserService extends BaseService {
  constructor(http: HttpClient) {
    super(http, 'User');
  }

  getPaged(pageIndex: number, pageSize: number, filter?: string): Observable<FluentResult<PagedResult<ReadUser>>> {
    let params = new HttpParams()
      .set('pageIndex', pageIndex)
      .set('pageSize', pageSize);

    if (filter) {
      params = params.set('filter', filter);
    }

    return this.get<PagedResult<ReadUser>>('GetPagging', params);
  }

  updateUser(id: string, dto: User): Observable<any> {
    return this.put(`Update?id=${id}`, dto);
  }

  deleteUser(id: string): Observable<any> {
    return this.delete(`Delete?id=${id}`);
  }
}
