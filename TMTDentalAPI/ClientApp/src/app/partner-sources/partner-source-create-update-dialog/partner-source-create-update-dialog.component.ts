import { Component, Input, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import {
  PartnerSourceService
} from "./../partner-source.service";


@Component({
  selector: "app-partner-source-create-update-dialog",
  templateUrl: "./partner-source-create-update-dialog.component.html",
  styleUrls: ["./partner-source-create-update-dialog.component.css"],
})
export class PartnerSourceCreateUpdateDialogComponent implements OnInit {
  myform: FormGroup;
  @Input() public id: string;
  title: string;
  submitted = false;

  get f() { return this.myform.controls; }

  constructor(
    private fb: FormBuilder,
    private partnerSourceService: PartnerSourceService,
    public activeModal: NgbActiveModal,
  ) {}

  ngOnInit() {
    this.myform = this.fb.group({
      name: ["", Validators.required],
      type: ['normal'],
    });

    if (this.id) {
      setTimeout(() => {
        this.partnerSourceService.get(this.id).subscribe((result) => {
          this.myform.patchValue(result);
        });
      });
    }
  }


  onSave() {
    this.submitted = true;

    if (!this.myform.valid) {
      return;
    }

    this.saveOrUpdate().subscribe(
      (result) => {
        if (result) {
          this.activeModal.close(result);
        } else {
          this.activeModal.close(true);
        }
      }, err => {
        console.log(err);
        this.submitted = false;
      });
  }

  saveOrUpdate() {
    var val = this.myform.value;
    if (!this.id) {
      return this.partnerSourceService.create(val);
    } else {
      return this.partnerSourceService.update(this.id, val);
    }
  }
}
