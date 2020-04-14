import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { FacebookMassMessagingService } from '../facebook-mass-messaging.service';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { switchMap, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { NotificationService } from '@progress/kendo-angular-notification';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FacebookMassMessagingScheduleDialogComponent } from '../facebook-mass-messaging-schedule-dialog/facebook-mass-messaging-schedule-dialog.component';
import { FacebookMassMessagingCreateUpdateDialogComponent } from '../facebook-mass-messaging-create-update-dialog/facebook-mass-messaging-create-update-dialog.component';
import { Subject } from 'rxjs';
import { FacebookTagsPaged, FacebookTagsService } from '../facebook-tags.service';
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
    
    this.searchTagUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadListTags();
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

  listAudienceFilter_Picker = [
    {
      type: 'Tag',
      name: 'Nhãn',
      formula_types: ['eq', 'neq'],
      formula_values: [],
      formula_displays: null,
    }, {
      type: 'Gender',
      name: 'Giới tính',
      formula_types: ['eq', 'neq'],
      formula_values: ['male', 'female'],
      formula_displays: ['Nam', 'Nữ'],
    }, {
      type: 'FirstName',
      name: 'Tên',
      formula_types: ['eq', 'neq', 'contains', 'doesnotcontain', 'startswith'],
      formula_values: [],
      formula_displays: null,
      inputbox: true
    }, {
      type: 'LastName',
      name: 'Họ',
      formula_types: ['eq', 'neq', 'contains', 'doesnotcontain', 'startswith'],
      formula_values: [],
      formula_displays: null,
      inputbox: true
    }
  ]
  selectedAudienceFilter_Picker: any;
  selectedFormula = {
    type: null, 
    value: null, 
    display: null
  };
  listAudienceFilter_Items: any[] = [];
  listTags: any[];
  inputSearchTag: string;
  searchTagUpdate = new Subject<string>();
  showButtonCreateTag: boolean = false;

  loadListTags() {
    var val = new FacebookTagsPaged();
    val.offset = 0;
    val.limit = 10;
    // console.log(this.inputSearchTag);
    val.search = this.inputSearchTag || '';
    this.facebookTagsService.getTags(val).subscribe(res => {
      this.listTags = res['items'];
      // console.log(this.listTags);
      if (this.listTags.length == 0) {
        this.showButtonCreateTag = true;
      } else {
        this.showButtonCreateTag = false;
      }
      this.listAudienceFilter_Picker[0].formula_values = [];
      for (let i = 0; i < this.listTags.length; i++) {
        this.listAudienceFilter_Picker[0].formula_values.push(this.listTags[i].name); // Add formula_values Tag
      }
      if (this.selectedAudienceFilter_Picker) {
        this.selectedAudienceFilter_Picker.formula_values = this.listAudienceFilter_Picker[0].formula_values;
      }
      // console.log(this.listTags);
    }, err => {
      console.log(err);
    })
  }
  createTag() {
    var val = {
      name: this.inputSearchTag
    };
    this.facebookTagsService.create(val).subscribe(res => {
      this.notificationService.show({
        content: 'Tạo nhãn thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadListTags();
      // console.log(res);
    }, err => {
      console.log(err);
    })
  }
  clickDropdownButtonAudienceFilter() {
    this.selectedAudienceFilter_Picker = null;
    this.selectedFormula.type = null;
    this.selectedFormula.value = null;
    this.selectedFormula.display = null;
    this.inputSearchTag = null;
    this.loadListTags();
    document.getElementById('dropdown-item-audience-filter').style.display = 'block';
  }
  selectAudienceFilter(item) {
    this.selectedAudienceFilter_Picker = item;
    this.selectedFormula.type = item.formula_types[0];
    document.getElementById('dropdown-item-audience-filter').style.display = 'none';
  }
  selectFormulaType(item) {
    this.selectedFormula.type = item;
    this.addAudienceFilter()
  }
  selectFormulaValue(item, i) {
    this.selectedFormula.value = item;
    if (this.selectedAudienceFilter_Picker.formula_displays) {
      this.selectedFormula.display = this.selectedAudienceFilter_Picker.formula_displays[i];
    } else {
      this.selectedFormula.display = null;
    }
    this.addAudienceFilter()
  }
  cancelInputFormulaValue() {
    this.selectedAudienceFilter_Picker = null;
  }
  saveInputFormulaValue(value) {
    this.selectedFormula.value = value;
    this.selectedFormula.display = value;
    this.addAudienceFilter()
  }
  addAudienceFilter() {
    if (this.selectedFormula.type && this.selectedFormula.value) {

      var temp = {
        type: this.selectedAudienceFilter_Picker.type,
        name: this.selectedAudienceFilter_Picker.name,
        formula_type: this.selectedFormula.type,
        formula_value: this.selectedFormula.value,
        formula_display: this.selectedFormula.display
      }
      this.listAudienceFilter_Items.push(temp);
      this.selectedAudienceFilter_Picker = null;
      this.selectedFormula.type = null;
      this.selectedFormula.value = null;
      this.selectedFormula.display = null;
      document.getElementById('dropdown-item-audience-filter').style.display = 'block';
      console.log(this.listAudienceFilter_Items);
      this.formGroup.patchValue({
        audienceFilter: this.convertAudienceFilterItemsToString()
      });
    }
  }
  selectAudienceFilterItem(index) {
    console.log("Hello");
  }
  deleteAudienceFilterItem(index) {
    this.listAudienceFilter_Items.splice(index, 1);
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
  convertAudienceFilterItemsToString() {
    var listAudienceFilter_Items_String = "";
    var element = "";
    for (let i = 0; i < this.listAudienceFilter_Items.length; i++) {
      element = (this.listAudienceFilter_Items[i].type || '') + ",|" +
        (this.listAudienceFilter_Items[i].name || '') + ",|" + 
        (this.listAudienceFilter_Items[i].formula_type || '') + ",|" + 
        (this.listAudienceFilter_Items[i].formula_value || '') + ",|" + 
        (this.listAudienceFilter_Items[i].formula_display || '');
      if (i === this.listAudienceFilter_Items.length - 1) {
        listAudienceFilter_Items_String += element;
      } else {
        listAudienceFilter_Items_String += element + ";|";
      }
    }
    this.convertAudienceFilterItemsToArray(listAudienceFilter_Items_String);
    return listAudienceFilter_Items_String;
  }
  convertAudienceFilterItemsToArray(listAudienceFilter_Items_String) {
    var listAudienceFilter_Items_Array = [];
    var element_s = listAudienceFilter_Items_String.split(";|");
    for (let i = 0; i < element_s.length; i++) {
      var element_sm = element_s[i].split(",|");
      var element = {
        type: element_sm[0],
        name: element_sm[1],
        formula_type: element_sm[2],
        formula_value: element_sm[3],
        formula_display: element_sm[4]
      }
      listAudienceFilter_Items_Array.push(element);
    }
    return listAudienceFilter_Items_Array;
  }
}
