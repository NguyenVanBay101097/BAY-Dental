import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
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

  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;

  id: string;
  saved: boolean = true;
  submitted = false;

  constructor(private fb: FormBuilder, 
    private route: ActivatedRoute, 
    private router: Router, 
    private commissionService: CommissionService, 
    private modalService: NgbModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
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

  get f() {
    return this.formGroup.controls;
  }

  get commissionProductRules() {
    return this.formGroup.get('commissionProductRules') as FormArray;
  }

  routeActive() {
    this.route.queryParams.subscribe(params => {
      this.id = params['id'];
    });

    if (this.id) {
      this.commissionService.get(this.id).subscribe(result => {
        this.formGroup.patchValue(result);

        const control = this.formGroup.get('commissionProductRules') as FormArray;
        control.clear();
        result['commissionProductRules'].forEach(line => {
          var g = this.fb.group(line);
          control.push(g);
        });
        this.formGroup.markAsPristine();
      }, err => {
        console.log(err);
      });
    } else {
      console.log("Tèo");
    } 
  }

  changeName() {
    this.saved = false;
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
    this.submitted = true;

    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;

    if (this.id) {
      this.commissionService.update(this.id, val)
      .subscribe(() => {
        this.saved = true;
      }, err => {
        console.log(err);
      });
    } else {
      this.commissionService.create(val)
      .subscribe(result => {
        this.router.navigate(['/commissions/form'], {
          queryParams: {
            id: result['id']
          },
        });
      }, err => {
        console.log(err);
      });
    }
  }
}
