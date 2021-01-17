import {
  PartnerSourceBasic,
  PartnerSourceService,
  PartnerSourcePaged,
} from "./../partner-source.service";
import { Component, OnInit, ViewChild, ElementRef, Input } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { Observable } from "rxjs";
import { AppSharedShowErrorService } from "src/app/shared/shared-show-error.service";


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


  constructor(
    private fb: FormBuilder,
    private partnerSourceService: PartnerSourceService,
    public activeModal: NgbActiveModal,
    private showErrorService: AppSharedShowErrorService
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
      },
    );
  }

  saveOrUpdate() {
    var val = this.myform.value;
    if (!this.id) {
      return this.partnerSourceService.create(val);
    } else {
      return this.partnerSourceService.update(this.id, val);
    }
  }

  get f() {
    return this.myform.controls;
  }
}
