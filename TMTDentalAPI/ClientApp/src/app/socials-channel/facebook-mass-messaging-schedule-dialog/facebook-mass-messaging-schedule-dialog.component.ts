import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { FacebookMassMessagingService } from '../facebook-mass-messaging.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-facebook-mass-messaging-schedule-dialog',
  templateUrl: './facebook-mass-messaging-schedule-dialog.component.html',
  styleUrls: ['./facebook-mass-messaging-schedule-dialog.component.css']
})
export class FacebookMassMessagingScheduleDialogComponent implements OnInit {
  formGroup: FormGroup;
  massMessagingId: string;
  constructor(private fb: FormBuilder, private massMessagingService: FacebookMassMessagingService,
    private intlService: IntlService, public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      scheduleDateObj: [null, Validators.required]
    });
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    var scheduleDate = this.intlService.formatDate(value.scheduleDateObj, 'yyyy-MM-ddTHH:mm:ss');
    var val = {
      massMessagingId: this.massMessagingId,
      scheduleDate: scheduleDate
    };

    this.massMessagingService.setScheduleDate(val).subscribe(() => {
      this.activeModal.close(true);
    });
  }
}
