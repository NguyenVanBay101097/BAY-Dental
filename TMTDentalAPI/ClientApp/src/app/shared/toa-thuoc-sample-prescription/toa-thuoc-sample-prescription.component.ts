import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { SamplePrescriptionsPaged, SamplePrescriptionsService, SamplePrescriptionsSimple } from 'src/app/sample-prescriptions/sample-prescriptions.service';

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

  constructor(private samplePrescriptionsService: SamplePrescriptionsService, ) { }

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
