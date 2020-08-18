import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareScenarioListComponent } from './tcare-scenario-list.component';

describe('TcareScenarioListComponent', () => {
  let component: TcareScenarioListComponent;
  let fixture: ComponentFixture<TcareScenarioListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareScenarioListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareScenarioListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
