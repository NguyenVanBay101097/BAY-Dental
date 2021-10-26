import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
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

  constructor(
    public activeModal: NgbActiveModal,
    private serviceCardsService: ServiceCardCardService,
    private fb: FormBuilder,
  ) { }


  ngOnInit(): void {
    this.formGroup = this.fb.group({
      fileName: [null, Validators.required],
    });
  }

  import() {
    this.serviceCardsService.actionImport(this.formData).subscribe((result: any) => {
      if (result.success) {
        this.activeModal.close(true);
      } else {
        this.errors = result.errors;
      }
    }, (err) => {
    });
  }

  onFileChange(file) {
    this.formData.append('file', file);
  }

  notifyError(value) {
    this.errors = value;
  }
}
