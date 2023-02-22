import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AgentCommmissionHistoryComponent } from './agent-commmission-history.component';

describe('AgentCommmissionHistoryComponent', () => {
  let component: AgentCommmissionHistoryComponent;
  let fixture: ComponentFixture<AgentCommmissionHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AgentCommmissionHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AgentCommmissionHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
