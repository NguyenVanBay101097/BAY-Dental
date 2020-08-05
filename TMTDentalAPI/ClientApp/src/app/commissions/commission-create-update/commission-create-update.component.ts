import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { CommissionService } from '../commission.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CommissionCreateUpdateDialogComponent } from '../commission-create-update-dialog/commission-create-update-dialog.component';
import { result } from 'lodash';

@Component({
  selector: 'app-commission-create-update',
  templateUrl: './commission-create-update.component.html',
  styleUrls: ['./commission-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class CommissionCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  saved: boolean = true;

  constructor(private fb: FormBuilder, 
    private route: ActivatedRoute, 
    private router: Router, 
    private commissionService: CommissionService, 
    private modalService: NgbModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: null,
      commissionProductRules: this.fb.array([])
    });
    this.routeActive();

  }

  getValueForm(key) {
    return this.formGroup.get(key).value;
  }

  getValueLineForm(line: FormGroup, key) {
    if (key == "name") {
      var appliedOn = line.get('appliedOn').value;
      switch (appliedOn) {
        case "3_global":
          return "Tất cả dịch vụ";
        case "2_product_category":
          return line.get('categ').value['name'];
        case "0_product_variant":
          return line.get('product').value['name'];
        default:
          return null;
      }
    }
    return line.get(key) ? line.get(key).value : null;
  }

  get commissionProductRules() {
    return this.formGroup.get('commissionProductRules') as FormArray;
  }

  routeActive() {
    this.route.queryParamMap.pipe(
      switchMap((params: ParamMap) => {
        this.id = params.get("id");
        if (this.id) {
          return this.commissionService.get(this.id);
        } else 
          return null;
      })).subscribe(result => {
        console.log(result);
        this.formGroup.patchValue(result);

        const control = this.formGroup.get('commissionProductRules') as FormArray;
        control.clear();
        result['commissionProductRules'].forEach(line => {
          var g = this.fb.group(line);
          control.push(g);
        });

        this.formGroup.markAsPristine();
        console.log(this.formGroup);
      });
  }

  addLine() {
    let modalRef = this.modalService.open(CommissionCreateUpdateDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.result.then(result => {
      let line = result as any;
      this.commissionProductRules.push(this.fb.group(line));
      this.commissionProductRules.markAsDirty();
      this.saved = false;
    }, () => {
    });
  }

  editLine(line: FormGroup) {
    let modalRef = this.modalService.open(CommissionCreateUpdateDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.line = line.value;
    modalRef.result.then(result => {
      line.patchValue(result);
      this.commissionProductRules.markAsDirty();
      this.saved = false;
    }, () => {
    });
  }

  deleteLine(i) {
    this.commissionProductRules.removeAt(i);
    this.commissionProductRules.markAsDirty();
    this.saved = false;
  }

  onSave() {
    var val = this.formGroup.value;
    this.commissionService.update(this.id, val)
    .subscribe(() => {
      this.saved = true;
    }, err => {
      console.log(err);
    });
  }
}
