import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { CardCardService } from 'src/app/card-cards/card-card.service';
import { ServiceCardCardService } from '../service-card-card.service';

@Component({
  selector: 'app-service-card-cards-preferential-import-dialog',
  templateUrl: './service-card-cards-preferential-import-dialog.component.html',
  styleUrls: ['./service-card-cards-preferential-import-dialog.component.css']
})
export class ServiceCardCardsPreferentialImportDialogComponent implements OnInit {
  @Input() title: string;
  errors: any = [];
  formData = new FormData();
  formGroup: FormGroup;
  isMemberCard = false;
  correctFormat = true;
  constructor(
    public activeModal: NgbActiveModal,
    private serviceCardsService: ServiceCardCardService,
    private fb: FormBuilder,
    private cardCardService: CardCardService,
    private notificationService: NotificationService,
  ) { }


  ngOnInit(): void {
    this.formGroup = this.fb.group({
      fileName: [null, Validators.required],
    });
  }

  import() {
    if (!this.correctFormat){
      this.notify('error','File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng');
      return;
    }
    if (this.isMemberCard){
      this.cardCardService.actionImport(this.formData).subscribe((result: any) =>{
        if (result.success) {
          this.notify("success","Import dữ liệu thành công");
          this.activeModal.close(true);
        } else {
          this.errors = result.errors;
        }
      })
    }else {
      this.serviceCardsService.actionImport(this.formData).subscribe((result: any) => {
        if (result.success) {
          this.notify("success","Import dữ liệu thành công");
          this.activeModal.close(true);
        } else {
          this.errors = result.errors;
        }
      }, (err) => {
      });
    }
  }

  onFileChange(file) {
    this.formData.append('file', file);
    this.errors = [];
  }

  notifyError(value) {
    this.errors = value;
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }
}
