import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AgentCreateUpdateDialogComponent } from './agent-create-update-dialog.component';

describe('AgentCreateUpdateDialogComponent', () => {
  let component: AgentCreateUpdateDialogComponent;
  let fixture: ComponentFixture<AgentCreateUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AgentCreateUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AgentCreateUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
