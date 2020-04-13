import { Component, OnInit } from '@angular/core';
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
export class FacebookMassMessagingCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  messaging = {};
  emoji: boolean = false;
  selectArea_start: number;
  selectArea_end: number;
  num_CharLeft: number = 640;

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

  listAudienceFilterPicker = [
    {
      type: 'Tag',
      name: 'Nhãn',
      formula_types: ['eq', 'neq'],
      formula_values: ['Tag_1', 'Tag_2', 'Tag_3', 'Tag_4', 'Tag_5', 'Tbg_6', 'Tbg_7'],
      formula_displays: null,
      search: true
    }, {
      type: 'Gender',
      name: 'Giới tính',
      formula_types: ['eq', 'neq'],
      formula_values: ['male', 'female'],
      formula_displays: ['Nam', 'Nữ'],
      search: false
    }, {
      type: 'FirstName',
      name: 'Tên',
      formula_types: ['eq', 'neq', 'contains', 'doesnotcontain', 'startswith'],
      formula_values: [],
      formula_displays: null,
      search: true
    }, {
      type: 'LastName',
      name: 'Họ',
      formula_types: ['eq', 'neq', 'contains', 'doesnotcontain', 'startswith'],
      formula_values: [],
      formula_displays: null,
      search: true
    }
  ]
  selectedAudienceFilterPicker: any;
  selectedFormulaType: any;
  selectedFormulaValue: any;
  selectedFormulaDisplay: any;
  listAudienceFilterItems: any[] = [];
  copyFormulaValues: any[] = [];
  clickDropdownButtonAudienceFilter() {
    this.selectedAudienceFilterPicker = null;
    this.selectedFormulaType = null;
    this.selectedFormulaValue = null;
    this.selectedFormulaDisplay = null;
    document.getElementById('dropdown-item-audience-filter').style.display = 'block';
  }
  selectAudienceFilter(item) {
    this.selectedAudienceFilterPicker = item;
    this.copyFormulaValues = this.selectedAudienceFilterPicker.formula_values;
    document.getElementById('dropdown-item-audience-filter').style.display = 'none';
  }
  selectFormulaType(item) {
    this.selectedFormulaType = item;
    this.addAudienceFilter()
  }
  selectFormulaValue(item, i) {
    this.selectedFormulaValue = item;
    if (this.selectedAudienceFilterPicker.formula_displays) {
      this.selectedFormulaDisplay = this.selectedAudienceFilterPicker.formula_displays[i];
    } else {
      this.selectedFormulaDisplay = null;
    }
    this.addAudienceFilter()
  }
  addAudienceFilter() {
    if (this.selectedFormulaType && this.selectedFormulaValue) {

      var temp = {
        type: this.selectedAudienceFilterPicker.type,
        name: this.selectedAudienceFilterPicker.name,
        formula_type: this.selectedFormulaType,
        formula_value: this.selectedFormulaValue,
        formula_display: this.selectedFormulaDisplay
      }
      this.listAudienceFilterItems.push(temp);
      this.selectedAudienceFilterPicker = null;
      this.selectedFormulaType = null;
      this.selectedFormulaValue = null;
      this.selectedFormulaDisplay = null;
      document.getElementById('dropdown-item-audience-filter').style.display = 'block';
      console.log(this.listAudienceFilterItems);
      this.formGroup.patchValue({
        audienceFilter: this.convertAudienceFilterItemsToString()
      });
    }
  }
  convertAudienceFilterItemsToString() {
    var listAudienceFilterItemsString = "";
    var element = "";
    for (let i = 0; i < this.listAudienceFilterItems.length; i++) {
      element = this.listAudienceFilterItems[i].type + ",|" +
        this.listAudienceFilterItems[i].name + ",|" + 
        this.listAudienceFilterItems[i].formula_type + ",|" + 
        this.listAudienceFilterItems[i].formula_value + ",|" + 
        this.listAudienceFilterItems[i].formula_display;
      if (i === this.listAudienceFilterItems.length - 1) {
        listAudienceFilterItemsString += element;
      } else {
        listAudienceFilterItemsString += element + ";|";
      }
    }
    this.convertAudienceFilterItemsToArray(listAudienceFilterItemsString);
    return listAudienceFilterItemsString;
  }
  convertAudienceFilterItemsToArray(listAudienceFilterItemsString) {
    var listAudienceFilterItemsArray = [];
    var element_s = listAudienceFilterItemsString.split(";|");
    for (let i = 0; i < element_s.length; i++) {
      var element_sm = element_s[i].split(",|");
      var element = {
        type: element_sm[0],
        name: element_sm[1],
        formula_type: element_sm[2],
        formula_value: element_sm[3],
        formula_display: element_sm[4]
      }
      listAudienceFilterItemsArray.push(element);
    }
    console.log(listAudienceFilterItemsArray);
  }
  deleteAudienceFilterItem(index) {
    this.listAudienceFilterItems.splice(index, 1);
  }
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
  searchFormulaValue(value) {
    if (value) {
      value = value.toLowerCase();
      this.copyFormulaValues = this.selectedAudienceFilterPicker.formula_values.filter(function(el: any) {
        return el.toLowerCase().indexOf(value) >= 0;
      });
    } else {
      this.copyFormulaValues = this.selectedAudienceFilterPicker.formula_values;
    }
  }
}
