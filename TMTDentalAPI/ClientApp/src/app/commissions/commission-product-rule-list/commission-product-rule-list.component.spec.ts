import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionProductRuleListComponent } from './commission-product-rule-list.component';

describe('CommissionProductRuleListComponent', () => {
  let component: CommissionProductRuleListComponent;
  let fixture: ComponentFixture<CommissionProductRuleListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionProductRuleListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionProductRuleListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
