import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/auth/auth.service';

@Component({
  selector: 'app-import-sample-data',
  templateUrl: './import-sample-data.component.html',
  styleUrls: ['./import-sample-data.component.css']
})
export class ImportSampleDataComponent implements OnInit {

  title = "Tạo dữ liệu mẫu"
  constructor(
    private activeModal: NgbActiveModal,
    private http: HttpClient,
    private authService: AuthService
  ) { }

  ngOnInit() {

  }

  importData(question) {
    var url = `${environment.apiDomain}api/web/ImportSimpleData`;
    var params = new HttpParams().set('question', question)
    if (question == "yes") {
      this.http.get(url, { params }).subscribe(
        () => {
          this.activeModal.close();
        }
      )
    }
    else {
      this.http.get(url, { params }).subscribe(
        () => {
          this.activeModal.close();
        }
      )
    }
  }

}
