import { Component, OnInit, SimpleChanges, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { FacebookMassMessagingService, AudienceFilterItem } from '../facebook-mass-messaging.service';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { switchMap, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { NotificationService } from '@progress/kendo-angular-notification';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FacebookMassMessagingScheduleDialogComponent } from '../facebook-mass-messaging-schedule-dialog/facebook-mass-messaging-schedule-dialog.component';
import { FacebookMassMessagingCreateUpdateDialogComponent } from '../facebook-mass-messaging-create-update-dialog/facebook-mass-messaging-create-update-dialog.component';
import { Subject } from 'rxjs';
import { FacebookTagsPaged, FacebookTagsService } from '../facebook-tags.service';
import { AudienceFilterTagComponent } from '../facebook-audience-filter-dropdown/audience-filter-tag/audience-filter-tag.component';
import { AudienceFilterGenderComponent } from '../facebook-audience-filter-dropdown/audience-filter-gender/audience-filter-gender.component';
import { AudienceFilterInputComponent } from '../facebook-audience-filter-dropdown/audience-filter-input/audience-filter-input.component';
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

  constructor(private fb: FormBuilder, private massMessagingService: FacebookMassMessagingService,
    private route: ActivatedRoute, private router: Router, private notificationService: NotificationService,
    private modalService: NgbModal, private facebookTagsService: FacebookTagsService) { }

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
        this.listAudienceFilter_Items = this.convertAudienceFilterItemsToArray(this.formGroup.value.audienceFilter);
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

  hello(value) {
    console.log(value);
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
      audienceFilter: this.convertAudienceFilterItemsToString()
    });

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

  //
  audience_filter_comp_data: { component, data };
  listAudienceFilter_Items: any[] = [];
  selectedAudienceFilter_Item: any;
  openAudienceFilter_Picker: boolean = false;
  openAudienceFilter: boolean = false;
  AudienceFilter_Item: AudienceFilterItem;
  listAudienceFilter_Picker = [
    {
      type: 'Tag',
      name: 'Nhãn'
    }, {
      type: 'Gender',
      name: 'Giới tính'
    }, {
      type: 'FirstName',
      name: 'Tên'
    }, {
      type: 'LastName',
      name: 'Họ'
    }
  ]
  convertFormulaType(item) {
    switch (item) {
      case 'eq':
        return 'bằng';
      case 'neq':
        return 'không bằng';
      case 'contains':
        return 'có chứa';
      case 'doesnotcontain':
        return 'không chứa';
      case 'startswith':
        return 'bắt đầu với';
      case 'bằng':
        return 'eq';
      case 'không bằng':
        return 'neq';
      case 'có chứa':
        return 'contains';
      case 'không chứa':
        return 'doesnotcontain';
      case 'bắt đầu với':
        return 'startswith';
    }
  }
  convertAudienceFilterItemsToString() {
    var result = {
      type: "and",
      items: this.listAudienceFilter_Items
    }
    return JSON.stringify(result);
  }
  convertAudienceFilterItemsToArray(listAudienceFilter_Items_String) {
    return JSON.parse(listAudienceFilter_Items_String).items;
  }
  clickedAudienceFilter_Item: boolean = false;
  clickAudienceFilter_Item(item) {
    if (this.AudienceFilter_Item !== item) {
      this.AudienceFilter_Item = item;
      switch (item.type) {
        case "Tag":
          this.audience_filter_comp_data = {
            component: AudienceFilterTagComponent,
            data: this.AudienceFilter_Item
          }
        break;
        case "Gender":
          this.audience_filter_comp_data = {
            component: AudienceFilterGenderComponent,
            data: this.AudienceFilter_Item
          }
        break;
        case "FirstName":
          this.audience_filter_comp_data = {
            component: AudienceFilterInputComponent,
            data: this.AudienceFilter_Item
          }
        break;
        case "LastName":
          this.audience_filter_comp_data = {
            component: AudienceFilterInputComponent,
            data: this.AudienceFilter_Item
          }
        break;
      } 
    }
    this.selectedAudienceFilter_Item = item;
    this.clickedAudienceFilter_Item = true;
  }
  count_clickOutside: number = 1;
  clickOutsideAudienceFilter_Item_Picker() {
    if (this.count_clickOutside === 2) {
      if (this.clickedAudienceFilter_Item === true) {
        this.clickedAudienceFilter_Item = false;
      } else {
        this.selectedAudienceFilter_Item = null;
      }
      this.count_clickOutside = 1;
    } else {
      this.count_clickOutside += 1;
    }
  }
  deleteAudienceFilterItem(index) {
    this.selectedAudienceFilter_Item = null;
    this.listAudienceFilter_Items.splice(index, 1);
  }
  clickedAudienceFilter: boolean = false;
  clickAudienceFilter() {
    this.openAudienceFilter = true;
    this.openAudienceFilter_Picker = false;
    this.clickedAudienceFilter = true;
  }
  clickOutsideAudienceFilter() {
    if (this.count_clickOutside === 2) {
      if (this.clickedAudienceFilter === true) {
        this.clickedAudienceFilter = false;
      } else {
        this.openAudienceFilter = false;
        this.openAudienceFilter_Picker = false;
      }
      this.count_clickOutside = 1;
    } else {
      this.count_clickOutside += 1;
    }
  }
  selectAudienceFilter(item) {
    this.AudienceFilter_Item = {
      type: item.type,
      name: item.name,
      formula_type: null,
      formula_value: null,
      formula_display: null
    }
    switch (item.type) {
      case "Tag":
        this.audience_filter_comp_data = {
          component: AudienceFilterTagComponent,
          data: this.AudienceFilter_Item
        }
        break;
      case "Gender":
        this.audience_filter_comp_data = {
          component: AudienceFilterGenderComponent,
          data: this.AudienceFilter_Item
        }
        break;
      case "FirstName":
        this.audience_filter_comp_data = {
          component: AudienceFilterInputComponent,
          data: this.AudienceFilter_Item
        }
        break;
      case "LastName":
        this.audience_filter_comp_data = {
          component: AudienceFilterInputComponent,
          data: this.AudienceFilter_Item
        }
        break;
    }
    document.getElementById("dropdown-item-audience-filter").style.display = 'none';
    this.openAudienceFilter_Picker = true;
  }
  addAudienceFilterItem() {
    if (this.AudienceFilter_Item.formula_type && this.AudienceFilter_Item.formula_value) {
      this.listAudienceFilter_Items.push(this.AudienceFilter_Item);
      this.openAudienceFilter = false;
      this.openAudienceFilter_Picker = false;
    }
  }
}
