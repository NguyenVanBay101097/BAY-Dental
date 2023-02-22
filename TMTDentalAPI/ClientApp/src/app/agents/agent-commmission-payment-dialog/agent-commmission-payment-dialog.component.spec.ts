import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AgentCommmissionPaymentDialogComponent } from './agent-commmission-payment-dialog.component';

describe('AgentCommmissionPaymentDialogComponent', () => {
  let component: AgentCommmissionPaymentDialogComponent;
  let fixture: ComponentFixture<AgentCommmissionPaymentDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AgentCommmissionPaymentDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AgentCommmissionPaymentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
