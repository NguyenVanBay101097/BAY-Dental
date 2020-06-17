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

interface Item {
  text: string;
  value: string;
}

@Component({
  selector: "app-partner-source-create-update-dialog",
  templateUrl: "./partner-source-create-update-dialog.component.html",
  styleUrls: ["./partner-source-create-update-dialog.component.css"],
})
export class PartnerSourceCreateUpdateDialogComponent implements OnInit {
  myform: FormGroup;
  filterdCategories: PartnerSourceBasic[];
  @ViewChild("form", { static: true }) formView: any;
  @ViewChild("nameInput", { static: true }) nameInput: ElementRef;
  @ViewChild("categCbx", { static: true }) categCbx: ComboBoxComponent;

  @Input() public id: string;
  title: string;
  submitted = false;
  public lstType: Array<Item> = [
    { text: "Bình thường", value: "normal" },
    { text: "Giới thiệu", value: "referral" },
  ];

  public selectedValue: string = "normal";

  constructor(
    private fb: FormBuilder,
    private partnerSourceService: PartnerSourceService,
    public activeModal: NgbActiveModal,
    private showErrorService: AppSharedShowErrorService
  ) {}

  ngOnInit() {
    this.myform = this.fb.group({
      name: ["", Validators.required],
      type: [null, Validators.required],
    });

    if (this.id) {
      setTimeout(() => {
        this.partnerSourceService.get(this.id).subscribe((result) => {
          this.myform.patchValue(result);
        });
      });
    }
  }

  // searchCategories(q?: string): Observable<PartnerSourceBasic[]> {
  //   var val = new PartnerSourcePaged();
  //   val.search = q;
  //   return this.partnerSourceService.autocomplete(val);
  // }

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
      (err) => this.showErrorService.show(err)
    );
  }

  saveOrUpdate() {
    var val = this.myform.value;
    val.type = val.type;
    if (!this.id) {
      return this.partnerSourceService.create(val);
    } else {
      return this.partnerSourceService.update(this.id, val);
    }
  }

  onCancel() {
    this.submitted = false;
    this.activeModal.close();
  }

  get f() {
    return this.myform.controls;
  }
}
