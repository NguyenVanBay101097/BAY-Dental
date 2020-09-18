import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { HrSalaryConfigService } from '../hr-salary-config.service';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { WorkEntryTypePage, WorkEntryTypeService } from 'src/app/work-entry-types/work-entry-type.service';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-hr-salary-config-create-update',
  templateUrl: './hr-salary-config-create-update.component.html',
  styleUrls: ['./hr-salary-config-create-update.component.css']
})
export class HrSalaryConfigCreateUpdateComponent implements OnInit {

  @ViewChild('typeCbx', { static: true }) typeCbx: ComboBoxComponent;

  listType: any;
  configForm: FormGroup;

  constructor(
    private hrSalaryConfigService: HrSalaryConfigService,
    private fb: FormBuilder,
    private workEntryTypeService: WorkEntryTypeService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.configForm = this.fb.group({
      id: null,
      companyId: [null, Validators.required],
      defaultGlobalLeaveTypeId: null,
      defaultGlobalLeaveType: [null, Validators.required]
    });

    this.loadListWorkEntryType();
    this.LoadData();

    this.typeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.typeCbx.loading = true)),
      switchMap(value => this.SearchType(value))
    ).subscribe((result: any) => {
      this.listType = result.items;
      this.typeCbx.loading = false;
    });
  }

  SearchType(value?: string) {
    const val = new WorkEntryTypePage();
    val.filter = value ? value : '';
    return this.workEntryTypeService.getPaged(val);
  }

  loadListWorkEntryType() {
    this.SearchType().subscribe((res: any) => {
      this.listType = res.items;
    });
  }

  LoadData() {
    this.hrSalaryConfigService.get().subscribe(res => {
      this.configForm.patchValue(res);
    });
  }

  onSaveOrUpdate() {
    if (!this.configForm.valid) {
      return false;
    }

    const val = this.configForm.value;
    val.defaultGlobalLeaveTypeId = val.defaultGlobalLeaveType.id;
    if (val.id) {
      this.hrSalaryConfigService.update(val.id, val).subscribe(res => {
        this.notificationService.show({
          content: 'Thành công!',
          hideAfter: 2000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    } else {
      this.hrSalaryConfigService.create(val).subscribe(res => {
        this.LoadData();
        this.notificationService.show({
          content: 'Thành công!',
          hideAfter: 2000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    }
  }
}


