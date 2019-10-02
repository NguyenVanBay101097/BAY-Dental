import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerSupplierCuDialogComponent } from './partner-supplier-cu-dialog.component';

describe('PartnerSupplierCuDialogComponent', () => {
  let component: PartnerSupplierCuDialogComponent;
  let fixture: ComponentFixture<PartnerSupplierCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerSupplierCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerSupplierCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
