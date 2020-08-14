import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrSalaryRuleComponent } from './hr-salary-rule.component';

describe('HrSalaryRuleComponent', () => {
  let component: HrSalaryRuleComponent;
  let fixture: ComponentFixture<HrSalaryRuleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrSalaryRuleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrSalaryRuleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
