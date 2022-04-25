import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HeaderCountdownComponent } from './header-countdown.component';

describe('HeaderCountdownComponent', () => {
  let component: HeaderCountdownComponent;
  let fixture: ComponentFixture<HeaderCountdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HeaderCountdownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HeaderCountdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
