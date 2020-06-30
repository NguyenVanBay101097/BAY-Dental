import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/auth/auth.service';
import { NotificationService } from '@progress/kendo-angular-notification';

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
    private authService: AuthService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {

  }

  importData(action) {
    var url = `${environment.apiDomain}api/web/ImportSampleData`;
    var params = new HttpParams().set('action', action)
    if (action == "Installed") {
      this.http.get(url, { params: params }).subscribe(
        () => {
          this.activeModal.close();
          this.notificationService.show({
            content: 'Khởi tạo dữ liệu mẫu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
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
