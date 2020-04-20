import { Component, OnInit, SimpleChanges, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { FacebookMassMessagingService } from '../facebook-mass-messaging.service';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { NotificationService } from '@progress/kendo-angular-notification';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FacebookMassMessagingScheduleDialogComponent } from '../facebook-mass-messaging-schedule-dialog/facebook-mass-messaging-schedule-dialog.component';
import { FacebookMassMessagingCreateUpdateDialogComponent } from '../facebook-mass-messaging-create-update-dialog/facebook-mass-messaging-create-update-dialog.component';
declare var $ :any;

@Component({
  selector: 'app-facebook-mass-messaging-create-update',
  templateUrl: './facebook-mass-messaging-create-update.component.html',
  styleUrls: ['./facebook-mass-messaging-create-update.component.css']
})
export class FacebookMassMessagingCreateUpdateComponent implements OnInit, OnChanges {
  formGroup: FormGroup;
  id: string;
  messaging = {};
  emoji: boolean = false;
  selectArea_start: number;
  selectArea_end: number;
  num_CharLeft: number = 640;
  audience_filter: any;
  showAudienceFilter: boolean = false;

  constructor(private fb: FormBuilder, private massMessagingService: FacebookMassMessagingService,
    private route: ActivatedRoute, private router: Router, private notificationService: NotificationService,
    private modalService: NgbModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      content: null,
      facebookPageId: null,
      audienceFilter: null
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
        this.audience_filter = this.formGroup.value.audienceFilter;
        this.showAudienceFilter = true;
      });

    this.formGroup.get('content').valueChanges.subscribe((value) => {
      if (value) {
        this.num_CharLeft = 640 - value.length;
      }
    });
    
    $(document).on('click', '.allow-focus', function (e) {
      e.stopPropagation();
    });

  }

  ngOnChanges(changes: SimpleChanges) {
    console.log(changes);
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
    this.formGroup.patchValue({
      audienceFilter: this.audience_filter
    });
    console.log(this.audience_filter);

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
  action_view(action_view_type) {
    let modalRef = this.modalService.open(FacebookMassMessagingCreateUpdateDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.massMessagingId = this.id;
    modalRef.componentInstance.massMessagingType = action_view_type;

    modalRef.result.then(() => {
      //
    }, () => {
    });
  }
  selectArea(event) {
    this.selectArea_start = event.target.selectionStart;
    this.selectArea_end = event.target.selectionEnd;
  }
  selectEmoji(event) {
    var icon_emoji = event.emoji.native;   
    if (this.formGroup.value.content) {
      this.formGroup.patchValue({
        content: this.formGroup.value.content.slice(0, this.selectArea_start) + icon_emoji + this.formGroup.value.content.slice(this.selectArea_end)
      });
    } else {
      this.formGroup.patchValue({
        content: icon_emoji
      });
    }
  }
  showEmoji() {
    this.emoji = true;
  }
  hideEmoji() {
    this.emoji = false;
  }
  saveAudienceFilter(event) {
    this.audience_filter = event;
    console.log(this.audience_filter);
  }
}
