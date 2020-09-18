import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrSalaryRuleCrudDialogComponent } from './hr-salary-rule-crud-dialog.component';

describe('HrSalaryRuleCrudDialogComponent', () => {
  let component: HrSalaryRuleCrudDialogComponent;
  let fixture: ComponentFixture<HrSalaryRuleCrudDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrSalaryRuleCrudDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrSalaryRuleCrudDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
