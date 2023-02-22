import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareScenarioCrUpComponent } from './tcare-scenario-cr-up.component';

describe('TcareScenarioCrUpComponent', () => {
  let component: TcareScenarioCrUpComponent;
  let fixture: ComponentFixture<TcareScenarioCrUpComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareScenarioCrUpComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareScenarioCrUpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
