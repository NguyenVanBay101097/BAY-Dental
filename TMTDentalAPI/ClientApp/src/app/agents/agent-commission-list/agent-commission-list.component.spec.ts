import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AgentCommissionListComponent } from './agent-commission-list.component';

describe('AgentCommissionListComponent', () => {
  let component: AgentCommissionListComponent;
  let fixture: ComponentFixture<AgentCommissionListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AgentCommissionListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AgentCommissionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
