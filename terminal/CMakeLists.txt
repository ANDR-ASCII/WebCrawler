cmake_minimum_required(VERSION 3.2)
set(CMAKE_SYSTEM_VERSION 7.0 CACHE TYPE INTERNAL FORCE)
project(${TerminalProjectName})

set(TerminalSourcesDirectory src)
set(TerminalIncludeDirectory include)

file(GLOB_RECURSE TerminalSourcesList "src/*.cpp")
file(GLOB_RECURSE TerminalIncludeList "include/*.h")

list(REMOVE_ITEM TerminalSourcesList src/stdafx.cpp)
add_msvc_precompiled_header("stdafx.h" "src/stdafx.cpp" TerminalSourcesList)

source_group("Precompiled Headers" FILES include/stdafx.h src/stdafx.cpp)

if(WIN32)
	source_group(WinResource FILES ${AppWin32Resoure})
endif()
	
configure_msvc_runtime()
add_executable(
	${TerminalProjectName}
	${TerminalSourcesList}
	${TerminalIncludeList}
)

add_definitions(${QT_DEFINITIONS})

if(MSVC)
	add_definitions(
		/EHsc
		/MP
		/permissive-
		/std:c++latest
		/Zi
		/W4
		/WX
	)
	
	set_target_properties(
		${TerminalProjectName}
		PROPERTIES COMPILE_FLAGS "/FI\"stdafx.h\""
	)
	
	set_target_properties(
		${TerminalProjectName} 
		PROPERTIES WIN32_EXECUTABLE YES
		LINK_FLAGS "/DEBUG /ENTRY:mainCRTStartup /MAP"
	)
endif()

include_directories(
	${TerminalIncludeDirectory}
	${QtXlsxWriterHeadersDir}
	${QtXlsxWriter_INCLUDE_DIRS}
	${Boost_INCLUDE_DIRS}
	${CMAKE_CURRENT_BINARY_DIR}
	${OPENSSL_INCLUDE_DIR}
)

target_link_libraries(
	${TerminalProjectName}
	${QtXlsxWriter_LIBS}
	${Boost_LIBRARIES}
	${Boost_SYSTEM_LIBRARY}
	${Boost_FILESYSTEM_LIBRARY}
	${Boost_ASIO_LIBRARY}
	${OPENSSL_LIBRARIES}
	Qt5::Widgets
	Qt5::Network
	Qt5::Svg
	Qt5::WebEngineWidgets
	Qt5::PrintSupport
)