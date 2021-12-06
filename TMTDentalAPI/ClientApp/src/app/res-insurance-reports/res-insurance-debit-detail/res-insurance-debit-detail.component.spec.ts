import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResInsuranceDebitDetailComponent } from './res-insurance-debit-detail.component';

describe('ResInsuranceDebitDetailComponent', () => {
  let component: ResInsuranceDebitDetailComponent;
  let fixture: ComponentFixture<ResInsuranceDebitDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResInsuranceDebitDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResInsuranceDebitDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
