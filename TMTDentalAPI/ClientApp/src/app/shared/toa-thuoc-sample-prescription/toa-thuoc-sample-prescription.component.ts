import { Component, OnInit, Output, EventEmitter, ViewChild, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { SamplePrescriptionsPaged, SamplePrescriptionsService, SamplePrescriptionsSimple } from 'src/app/sample-prescriptions/sample-prescriptions.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { NgbModal, NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-toa-thuoc-sample-prescription',
  templateUrl: './toa-thuoc-sample-prescription.component.html',
  styleUrls: ['./toa-thuoc-sample-prescription.component.css']
})
export class ToaThuocSamplePrescriptionComponent implements OnInit, OnChanges {

  search: string;
  name: string;
  invalid: boolean = false;
  samplePrescriptions_root: SamplePrescriptionsSimple[] = [];
  samplePrescriptions: SamplePrescriptionsSimple[] = [];
  @Input() item: any;
  @Output() nameSamplePrescription = new EventEmitter<string>();
  @Output() itemSamplePrescription = new EventEmitter<string>();
  @ViewChild('myDrop', { static: true }) myDrop: NgbDropdown;

  constructor(private samplePrescriptionsService: SamplePrescriptionsService, 
    private errorService: AppSharedShowErrorService, private modalService: NgbModal) { }

  ngOnInit() {
    setTimeout(() => {
      this.loadSamplePrescriptionsList();
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.item) {
      if (this.item) {
        this.samplePrescriptions.splice(0, 0, this.item);
        this.name = null;
      }
    }
  }

  loadSamplePrescriptionsList() {
    var val = new SamplePrescriptionsPaged();
    this.samplePrescriptionsService.getPaged(val).subscribe(result => {
      this.samplePrescriptions_root = result.items;
      this.samplePrescriptions = result.items;
    });
  }

  searchItem() {
    this.samplePrescriptions = this.samplePrescriptions_root.filter(x => x.name.search(this.search) >= 0);
  }

  saveItem() {
    if (!this.name) {
      this.invalid = true;
      return;
    }
    this.nameSamplePrescription.emit(this.name);
  }

  selectItem(item) {
    this.itemSamplePrescription.emit(item);
    this.myDrop.close();
  }

  toggledDropdown(event) {
    if (event) {
      this.invalid = false;
      this.name = null;
      this.loadSamplePrescriptionsList();
    }
  }
}
