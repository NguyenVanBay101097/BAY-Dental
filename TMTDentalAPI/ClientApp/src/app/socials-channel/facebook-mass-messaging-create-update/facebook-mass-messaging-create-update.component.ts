import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { FacebookMassMessagingService } from '../facebook-mass-messaging.service';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { NotificationService } from '@progress/kendo-angular-notification';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FacebookMassMessagingScheduleDialogComponent } from '../facebook-mass-messaging-schedule-dialog/facebook-mass-messaging-schedule-dialog.component';

@Component({
  selector: 'app-facebook-mass-messaging-create-update',
  templateUrl: './facebook-mass-messaging-create-update.component.html',
  styleUrls: ['./facebook-mass-messaging-create-update.component.css']
})
export class FacebookMassMessagingCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  messaging = {};

  constructor(private fb: FormBuilder, private massMessagingService: FacebookMassMessagingService,
    private route: ActivatedRoute, private router: Router, private notificationService: NotificationService,
    private modalService: NgbModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      content: null,
      facebookPageId: null
    });

    this.route.queryParamMap.pipe(
      switchMap((params: ParamMap) => {
        this.id = params.get("id");
        if (this.id) {
          return this.massMessagingService.get(this.id);
        } else {
          return this.massMessagingService.defaultGet();
        }
      })).subscribe((result: any) => {
        this.messaging = result;
        this.formGroup.patchValue(result);
      });
  }

  loadRecord() {
    if (this.id) {
      return this.massMessagingService.get(this.id).subscribe((result: any) => {
        this.messaging = result;
        this.formGroup.patchValue(result);
      });
    }
  }

  createNew() {
    this.router.navigate(['/facebook-management/mass-messagings/form'], { queryParams: {} });
  }

  actionSchedule() {
    if (this.id) {
      let modalRef = this.modalService.open(FacebookMassMessagingScheduleDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.massMessagingId = this.id;

      modalRef.result.then(() => {
        this.loadRecord();
      }, () => {
      });
    } else {
      if (!this.formGroup.valid) {
        return false;
      }

      var val = this.formGroup.value;

      this.massMessagingService.create(val).subscribe((result: any) => {
        this.router.navigate(['/facebook-management/mass-messagings/form'], { queryParams: { id: result.id } });

        let modalRef = this.modalService.open(FacebookMassMessagingScheduleDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.massMessagingId = result.id;

        modalRef.result.then(() => {
          this.loadRecord();
        }, () => {
        });
      });
    }
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    if (this.id) {
      this.massMessagingService.update(this.id, val).subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    } else {
      this.massMessagingService.create(val).subscribe((result: any) => {
        this.router.navigate(['/facebook-management/mass-messagings/form'], { queryParams: { id: result.id } });
      });
    }
  }

  actionCancel() {
    if (this.id) {
      this.massMessagingService.actionCancel([this.id]).subscribe((result: any) => {
        this.loadRecord();
      });
    }
  }

  actionSend() {
    if (this.id) {
      this.massMessagingService.actionSend([this.id]).subscribe(() => {
        this.notificationService.show({
          content: 'Gửi thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    }
    else {
      if (!this.formGroup.valid) {
        return false;
      }

      var val = this.formGroup.value;

      this.massMessagingService.create(val).subscribe((result: any) => {
        this.router.navigate(['/facebook-management/mass-messagings/form'], { queryParams: { id: result.id } });

        this.massMessagingService.actionSend([result.id]).subscribe(() => {
          this.loadRecord();
        });
      });
    }
  }
}
