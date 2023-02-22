import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionProductRuleDialogComponent } from './commission-product-rule-dialog.component';

describe('CommissionProductRuleDialogComponent', () => {
  let component: CommissionProductRuleDialogComponent;
  let fixture: ComponentFixture<CommissionProductRuleDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionProductRuleDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionProductRuleDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
