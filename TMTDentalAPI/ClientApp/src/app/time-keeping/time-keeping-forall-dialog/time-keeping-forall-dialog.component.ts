import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { WorkEntryTypeService, WorkEntryTypePage } from 'src/app/work-entry-types/work-entry-type.service';
import { Observable, BehaviorSubject, from } from 'rxjs';
import { delay, switchMap, map, tap, debounceTime } from 'rxjs/operators';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-time-keeping-forall-dialog',
  templateUrl: './time-keeping-forall-dialog.component.html',
  styleUrls: ['./time-keeping-forall-dialog.component.css']
})
export class TimeKeepingForallDialogComponent implements OnInit {

  @ViewChild('typeCbx', { static: true }) typeCbx: ComboBoxComponent;
  formGroup: FormGroup;
  typeList: any;
  title: string;

  constructor(
    private fb: FormBuilder,
    private entryTypeService: WorkEntryTypeService,
    public activeModal: NgbActiveModal,
    private intelservice: IntlService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      date: [null, Validators.required],
      workEntryType: [null, Validators.required],
      workEntryTypeId: null
    });

    setTimeout(() => {
      this.loadListTypePaged();
    });
    this.typeCbx.filterChange.asObservable().pipe(
      switchMap((value: any) => this.searchEntryType(value).pipe(
        tap(() => this.typeCbx.loading = true),
        debounceTime(300)
      ))
    ).subscribe(x => {
      this.typeList = x.items;
      this.typeCbx.loading = false;
    });
  }

  loadListTypePaged() {
    this.searchEntryType().subscribe(res => {
      console.log(res);
      this.typeList = res.items;
    });
  }

  searchEntryType(value?: string) {
    const val = new WorkEntryTypePage();
    val.filter = value || '';
    return this.entryTypeService.getPaged(val);
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }
    const val = this.formGroup.value;
    val.date = this.intelservice.formatDate(val.date, 'yyyy-MM-dd HH:mm:ss');
    val.workEntryTypeId  = val.workEntryType.id;
    this.activeModal.close(val);
  }

}
