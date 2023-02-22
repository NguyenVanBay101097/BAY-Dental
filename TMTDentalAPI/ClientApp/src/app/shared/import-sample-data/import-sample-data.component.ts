import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'src/app/auth/auth.service';
import { WebService } from 'src/app/core/services/web.service';

@Component({
  selector: 'app-import-sample-data',
  templateUrl: './import-sample-data.component.html',
  styleUrls: ['./import-sample-data.component.css']
})
export class ImportSampleDataComponent implements OnInit {

  title = "Tạo dữ liệu mẫu"
  constructor(
    public activeModal: NgbActiveModal,
    public authService: AuthService,
    private webService: WebService
  ) { }

  ngOnInit() {

  }

  importData(action) {
    var params = new HttpParams().set('action', action)
    if (action == "Installed") {
      this.webService.impottSampleData(params).subscribe(
        () => {
          this.activeModal.close(true);
        }
      )
    }
    else {
      this.webService.impottSampleData(params).subscribe(
        () => {
          this.activeModal.close(false);
        }
      )
    }
  }

}
