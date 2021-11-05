import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class IrAttachmentService {
  apiUrl = 'api/IrAttachments';

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  deleteImage(id) {
    return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
  }
}
