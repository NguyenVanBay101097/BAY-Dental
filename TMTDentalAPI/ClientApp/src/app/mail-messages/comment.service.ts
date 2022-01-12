import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";

export class CreateCommentRequest{
    body: string;
    threadId: string;
    threadModel: string;
}

@Injectable({ providedIn: 'root' })
export class CommentService {
    apiUrl = 'api/Comments';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    create(val: CreateCommentRequest) {
        return this.http.post(this.baseApi + this.apiUrl,val);
    }
   }