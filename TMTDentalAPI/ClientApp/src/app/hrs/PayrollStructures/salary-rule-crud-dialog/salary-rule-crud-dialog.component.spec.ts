import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SalaryRuleCrudDialogComponent } from './salary-rule-crud-dialog.component';

describe('SalaryRuleCrudDialogComponent', () => {
  let component: SalaryRuleCrudDialogComponent;
  let fixture: ComponentFixture<SalaryRuleCrudDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SalaryRuleCrudDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SalaryRuleCrudDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
