import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerAutoGenerateCodeDialogComponent } from './partner-customer-auto-generate-code-dialog.component';

describe('PartnerCustomerAutoGenerateCodeDialogComponent', () => {
  let component: PartnerCustomerAutoGenerateCodeDialogComponent;
  let fixture: ComponentFixture<PartnerCustomerAutoGenerateCodeDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerAutoGenerateCodeDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerAutoGenerateCodeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
