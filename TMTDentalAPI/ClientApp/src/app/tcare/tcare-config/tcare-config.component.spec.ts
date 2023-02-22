import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareConfigComponent } from './tcare-config.component';

describe('TcareConfigComponent', () => {
  let component: TcareConfigComponent;
  let fixture: ComponentFixture<TcareConfigComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareConfigComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
