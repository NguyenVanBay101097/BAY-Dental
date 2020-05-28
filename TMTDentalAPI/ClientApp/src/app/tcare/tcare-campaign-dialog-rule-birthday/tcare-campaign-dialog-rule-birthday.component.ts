import { Component, OnInit } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { FormBuilder, FormGroup } from "@angular/forms";
import { TcareService } from "../tcare.service";

@Component({
  selector: "app-tcare-campaign-dialog-rule-birthday",
  templateUrl: "./tcare-campaign-dialog-rule-birthday.component.html",
  styleUrls: ["./tcare-campaign-dialog-rule-birthday.component.css"],
})
export class TcareCampaignDialogRuleBirthdayComponent implements OnInit {
  title: string;
  cell: any;
  formGroup: FormGroup;

  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private tcareService: TcareService
  ) {}

  ngOnInit() {
    this.formGroup = this.fb.group({
      beforeDays: [0],
    });

    if (this.cell.id) this.loadFormApi();
  }

  loadFormApi() {
    if (this.cell && this.cell.id) {
      this.tcareService
        .getTcareRuleBirthday(this.cell.id)
        .subscribe((result) => {
          console.log(result);
          this.formGroup.patchValue(result);
        });
    }
  }

  onSave() {
    if (this.formGroup.invalid || !this.cell || !this.cell.id) return false;

    var value = this.formGroup.value;
    this.tcareService
      .updateTCareRuleBirthday(this.cell.id, value)
      .subscribe((retult) => {
        console.log("thành công");
        this.activeModal.close();
      });
  }
}
